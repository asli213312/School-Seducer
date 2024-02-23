mergeInto(LibraryManager.library, {

    Hello: function () {
        window.alert("Hello, world! from UNITY SCRIPT");
    },

    HelloString: function (str) {
        window.alert(UTF8ToString(str));
    },

    RetrieveNutakuUser: function () {                
        Nutaku.retrieveUser(function(resultData) 
        {
            console.log(gameInstance + "found build in UNITY script");
            gameInstance.SendMessage("_SG_NutakuManager", "OnUserRetrieved", JSON.stringify(resultData));
            
            var grade = resultData["grade"];
            
            if (grade != 0)
            {
                console.log("THIS USER HAS AN ACCOUNT");
            }
            else 
            {
                console.log("NO ACCOUNT");
                // add guest play stuff
            }
        });
    }
});