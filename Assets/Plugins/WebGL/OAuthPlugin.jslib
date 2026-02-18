mergeInto(LibraryManager.library, {
  OpenOAuthWindow: function (urlPtr) {
    var url = UTF8ToString(urlPtr);

    console.log("[OAuth Plugin] Opening OAuth popup");
    console.log("[OAuth Plugin] URL:", url);

    // 팝업 창 크기 및 위치
    var width = 500;
    var height = 600;
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;

    // OAuth 팝업 열기
    var popup = window.open(
      url,
      "OAuth Login",
      "width=" +
        width +
        ",height=" +
        height +
        ",left=" +
        left +
        ",top=" +
        top +
        ",toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes",
    );

    if (!popup) {
      console.error(
        "[OAuth Plugin] Failed to open popup. Check popup blocker!",
      );
      SendMessage("OAuthCallbackReceiver", "OnOAuthError", "Popup blocked");
      return;
    }

    console.log("[OAuth Plugin] Popup opened successfully");

    // 팝업에서 메시지 수신 대기
    var messageHandler = function (event) {
      console.log("[OAuth Plugin] Message received from:", event.origin);
      console.log("[OAuth Plugin] Message type:", event.data.type);

      if (event.data.type === "oauth_success") {
        console.log("[OAuth Plugin] ✓ Success! Token received");

        // Unity로 토큰 전달
        SendMessage(
          "OAuthCallbackReceiver",
          "OnOAuthSuccess",
          event.data.token,
        );

        // 팝업 닫기
        if (popup && !popup.closed) {
          popup.close();
        }

        // 이벤트 리스너 제거
        window.removeEventListener("message", messageHandler);
      } else if (event.data.type === "oauth_error") {
        console.log("[OAuth Plugin] ✗ Error:", event.data.error);

        // Unity로 에러 전달
        SendMessage("OAuthCallbackReceiver", "OnOAuthError", event.data.error);

        // 팝업 닫기
        if (popup && !popup.closed) {
          popup.close();
        }

        // 이벤트 리스너 제거
        window.removeEventListener("message", messageHandler);
      }
    };

    // 메시지 이벤트 리스너 등록
    window.addEventListener("message", messageHandler);

    // 팝업이 닫혔는지 주기적으로 확인
    var checkClosed = setInterval(function () {
      if (!popup || popup.closed) {
        clearInterval(checkClosed);
        window.removeEventListener("message", messageHandler);
        console.log("[OAuth Plugin] Popup was closed");
      }
    }, 1000);
  },
});
