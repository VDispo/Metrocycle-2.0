mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },
  SaveStats: function (scenename,
        speed,
        elapsedTime,
        errors
    ) {

    scenename = UTF8ToString(scenename);
    const stats = {
        "AvgSpeed": speed,
        "ElapsedTime": elapsedTime,
        "Errors": errors,
        "DateTime": new Date(),
    };

    let curStats = JSON.parse(localStorage.getItem(scenename)) || [];
    curStats.push(stats);

    localStorage.setItem(scenename, JSON.stringify(curStats));
  },
});
