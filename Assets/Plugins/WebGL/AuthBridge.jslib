mergeInto(LibraryManager.library, {
  // Unity가 준비됐음을 웹 페이지에 알림
  NotifyUnityReady: function () {
    if (window.onUnityReady) {
      window.onUnityReady();
    }
  },

  // 로그아웃 시 웹 페이지 로그아웃 호출
  LogoutFromBrowser: function () {
    if (window.logoutFromPage) {
      window.logoutFromPage();
    }
  },

  // LocalStorage에서 토큰 직접 읽기 (보조 수단)
  GetStoredToken: function () {
    var token = localStorage.getItem("auth_token") || "";
    var size = lengthBytesUTF8(token) + 1;
    var buf = _malloc(size);
    stringToUTF8(token, buf, size);
    return buf;
  },
});
