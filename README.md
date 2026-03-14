# Metamong Client

Unity WebGL 기반 메타버스 클라이언트. OAuth 인증 후 Colyseus WebSocket으로 실시간 멀티플레이를 지원합니다.

---

## 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Core/
│   │   └── ServerSchemas.cs          # 공유 DTO, Enum (RC, UserStatus, MBTI 등)
│   ├── Managers/
│   │   ├── Managers.cs               # 싱글톤 Facade, 브라우저 → Unity 토큰 수신
│   │   └── AuthManager.cs            # [레거시] FastAPIHandler로 마이그레이션 예정
│   ├── Network/
│   │   ├── NetworkCore.cs            # 네트워크 Facade 싱글톤
│   │   ├── NetworkRunner.cs          # MonoBehaviour 진입점
│   │   ├── FastAPIHandler.cs         # REST API 클라이언트 (JWT 인증, 유저 관리)
│   │   ├── ColyseusHandler.cs        # WebSocket 실시간 핸들러
│   │   ├── NetworkSetting.cs         # ScriptableObject 설정
│   │   ├── INetProvider.cs           # 네트워크 인터페이스
│   │   ├── DTOs/
│   │   │   └── Vector2Serialized.cs  # 위치 DTO
│   │   └── Schemas/
│   │       ├── MyRoomState.cs        # Colyseus 룸 상태
│   │       └── Player.cs            # Colyseus 플레이어 엔티티
│   └── Player/
│       ├── PlayerManager.cs          # 플레이어 컨트롤러, 서버 위치 구독
│       ├── PlayerMoveService.cs      # 클라이언트 예측 + 서버 Reconciliation
│       └── PlayerDomain.cs          # 플레이어 상태 모델
├── Editor/
│   └── WebGLBuildPostProcessor.cs    # 빌드 시 index.html에 BACKEND_URL 주입
├── Plugins/WebGL/
│   └── AuthBridge.jslib              # JS ↔ C# 브릿지 (토큰 교환, 로그아웃)
└── WebGLTemplates/LoginTemplate/
    └── index.html                    # OAuth 로그인 UI + Unity 로더
```

---

## 네트워크 아키텍처

```
Browser (index.html)
  ├── OAuth 로그인 (Google / Kakao / Naver)
  │     └── 백엔드 리다이렉트 → 토큰 수신
  └── SendMessage("@Managers", "OnReceiveAuthToken", token)
              ↓
      Managers.OnReceiveAuthToken()
              ↓
      NetworkCore (Facade Singleton)
        ├── FastAPIHandler ──► api.jangmyun.dev  (REST/HTTPS)
        │     ├── GET  /api/users/me
        │     ├── PATCH /api/users/me/initialize
        │     └── PATCH /api/users/me/rc
        └── ColyseusHandler ─► ws.jangmyun.dev  (WebSocket)
              ├── JoinRoom (JWT 인증)
              ├── SendMove (Vector2)
              └── OnStateChange → PlayerManager.Reconcile()
```

---

## 인증 흐름

1. `index.html` 로드 → `localStorage`에 토큰 존재 시 유효성 검증
2. 토큰 없음 → OAuth 버튼 클릭 → `{BACKEND_URL}/api/auth/login/{provider}` 리다이렉트
3. 백엔드 OAuth 처리 후 `?token=...` 쿼리 파라미터로 복귀
4. 브라우저가 토큰을 `localStorage`에 저장하고 Unity 로드
5. Unity 준비 완료 → `NotifyUnityReady()` (jslib) → `onUnityReady()` (JS)
6. `SendMessage` → `Managers.OnReceiveAuthToken(token)`
7. `NetworkCore.ReceiveToken()` → `FastAPIHandler.FetchMe()` (`GET /api/users/me`)
8. 로그인 완료 이벤트 → Colyseus 룸 참여

---

## 위험 요소 및 개선사항

### 🔴 Critical

#### 1. JWT 토큰 Git 커밋 노출
`NetworkSetting.asset`에 실제 JWT 토큰이 하드코딩되어 리포지토리에 커밋됨.

- **조치**: 즉시 토큰 무효화, `.asset` 파일에서 jwt 필드 제거, `.gitignore` 설정
- **히스토리 정리**: `git filter-branch` 또는 BFG Repo Cleaner 사용 권장

#### 2. OAuth 토큰 URL 파라미터 전달
```javascript
// index.html
const tokenFromUrl = urlParams.get("token"); // ?token=eyJ...
```
브라우저 히스토리, 서버 액세스 로그, Referer 헤더에 토큰이 노출됨.

- **조치**: 서버 세션 쿠키 방식으로 교체하거나, `postMessage` + 짧은 수명 코드 교환 방식 도입

#### 3. WebSocket 비암호화 (ws://)
```yaml
# NetworkSetting.asset
colyseusServerUrl: ws.jangmyun.dev
useSecureConnection: 0
```
프로덕션에서 플레이어 위치 및 JWT가 평문 전송됨.

- **조치**: `wss://` 강제 설정, `useSecureConnection = true`

---

### 🟠 High

#### 4. 초기화 순서 Race Condition
`NetworkRunner.Awake()`에서 `NetworkCore.Initialize()`가 실행되기 전에 브라우저로부터 토큰이 수신될 경우 `_fastAPIHandler`가 null 상태.

- **조치**: `NetworkCore.ReceiveToken()`에 초기화 완료 여부 체크 추가, 미초기화 시 토큰 큐잉

#### 5. `async Task Start()` 예외 무시
```csharp
// NetworkRunner.cs
async Task Start()  // Unity는 반환값을 추적하지 않음
{
    await NetworkCore.Instance.JoinSquare();
}
```
Colyseus 연결 실패 시 예외가 **완전히 무시**됨.

- **조치**: `async void Start()`로 변경하거나 try-catch로 예외 핸들링 추가

#### 6. Colyseus 재연결 로직 부재
`OnLeave`, `OnError` 이벤트 핸들러 없음. 네트워크 단절 시 복구 불가.

- **조치**: 이벤트 바인딩 + Exponential Backoff 재연결 로직 추가

---

### 🟡 Medium

#### 7. NetworkSetting 필드 미구현
```csharp
// 선언은 되어 있으나 실제로 사용되지 않음
public int requestTimeout = 10;    // UnityWebRequest.timeout 미설정
public int maxRetryCount = 3;      // 재시도 로직 없음
public int simulatedLatencyMs = 100; // 미구현
```
- **조치**: `FastAPIHandler`에서 `www.timeout = _settings.requestTimeout` 적용, 재시도 로직 구현

#### 8. InputHistory Queue 무한 증가
```csharp
// PlayerMoveService.cs
private readonly int _maxHistoryCount = 100; // 선언만, 적용 안 됨
_domain.InputHistory.Enqueue(new InputFrame { dir, dt });
// Dequeue 없음, Reconcile 후에도 클리어 없음
```
- **조치**: Enqueue 전 크기 체크 추가, Reconcile 후 히스토리 클리어

#### 9. SendMove 호출 빈도 제한 없음
매 프레임 `NetworkCore.Instance.SendMove()` 호출 → 60fps 기준 초당 60개 WebSocket 메시지.

- **조치**: 틱레이트 기반 전송 (예: 20tick/s), 변경이 있을 때만 전송

#### 10. 설정 소스 이중화 (이중 진실 문제)
- `NetworkSetting.asset` (NetworkCore 사용)
- `Resources/Configs/ENV.asset` + `AppConfig.cs` (별도 싱글톤)

어떤 URL이 실제로 사용되는지 환경별로 추적이 어려움.

- **조치**: 하나의 설정 소스로 통합

---

### 🔵 Low

#### 11. AuthManager.cs 레거시 잔존
FastAPIHandler로 마이그레이션 완료 후에도 파일이 남아 있어 혼란 유발.

- **조치**: 마이그레이션 완료 확인 후 삭제

#### 12. Colyseus Room Cleanup 없음
씬 언로드 시 `_room.Leave()` 호출 없음 → 서버에 고스트 플레이어 잔존.

- **조치**: `OnDestroy()`에서 `_room?.Leave()` 호출

---

## 우선순위 요약

| 우선순위 | 항목 |
|---------|------|
| **P0** | NetworkSetting.asset JWT 제거 + 토큰 무효화 |
| **P0** | URL 파라미터 토큰 전달 방식 변경 |
| **P1** | wss:// 강제 설정 |
| **P1** | Race Condition 해결 (토큰 수신 타이밍) |
| **P1** | `async Task Start()` 예외 처리 |
| **P1** | Colyseus 재연결 로직 추가 |
| **P2** | requestTimeout / maxRetryCount 적용 |
| **P2** | InputHistory 크기 제한 |
| **P2** | SendMove 레이트 리미팅 |
| **P3** | AuthManager.cs 삭제 |
| **P3** | ENV.asset / NetworkSetting 통합 |
| **P3** | ColyseusHandler OnDestroy 정리 |
