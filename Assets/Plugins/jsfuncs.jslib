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

  GetStatsForScene: function (scenename) {
    scenename = UTF8ToString(scenename);
    let curStats = JSON.parse(localStorage.getItem(scenename)) || [];

    // convert dates to unix timestamp for easier transport to C#
    for (let idx = 0; idx < curStats.length; ++idx) {
        curStats[idx]["DateTime"] = new Date(curStats[idx]["DateTime"]).getTime();
    }

   let returnStr = JSON.stringify(curStats);
   console.log("Stats for " + scenename + ": " + returnStr);

   let bufferSize = lengthBytesUTF8(returnStr) + 1;
   let buffer = _malloc(bufferSize);
   stringToUTF8(returnStr, buffer, bufferSize);
   return buffer
  },
});
