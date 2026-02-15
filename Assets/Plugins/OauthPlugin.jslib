mergeInto(LibraryManager.library, {
  OpenOAuthPopup: function (url, callbackObjectName, provider) {
    var url = "https://127.0.0.1/auth/" + Pointer_stringify(provider);
    window.open(url, "OAuth Login", "width=500,height=600");
  },
});
