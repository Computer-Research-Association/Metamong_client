mergeInto(LibraryManager.library, {
  OpenOAuthWindow: function (urlPtr) {
    var url = UTF8ToString(urlPtr);

    // 팝업 창 설정
    var width = 500;
    var height = 600;
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;

    // OAuth 팝업 열기
    var popup = window.open(
      url,
      "OAuth Login",
      "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top,
    );

    // 팝업에서 메시지 수신 대기
    window.addEventListener("message", function (event) {
      // 보안: origin 검증
      if (event.origin !== "http://localhost:8000") {
        return;
      }

      if (event.data.type === "oauth_success") {
        // Unity로 토큰 전달
        SendMessage("OAuthManager", "OnOAuthSuccess", event.data.token);
        if (popup) popup.close();
      } else if (event.data.type === "oauth_error") {
        SendMessage("OAuthManager", "OnOAuthError", event.data.error);
        if (popup) popup.close();
      }
    });
  },
});
