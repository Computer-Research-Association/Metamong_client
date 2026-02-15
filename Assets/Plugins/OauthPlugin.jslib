mergeInto(LibraryManager.library, {
  OpenOAuthPopup: function (provider) {
    var url = "https://your-server.com/auth/" + Pointer_stringify(provider);
    window.open(url, "OAuth Login", "width=500,height=600");
  },
});
