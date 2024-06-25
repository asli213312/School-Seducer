mergeInto(LibraryManager.library, {

    Alert: function (str) {
        window.alert(UTF8ToString(str) + " from UNITY SCRIPT");
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
    },
    
    RequestPayment: function (paymentData, sessionTicket, playfabId) 
    {
      Nutaku.sessionTicket = UTF8ToString(sessionTicket);
      Nutaku.playFabId = UTF8ToString(playfabId);

      console.log("Find playfabid: " + Nutaku.playFabId);
      console.log("Find sessionTicket: " + Nutaku.sessionTicket);

      Nutaku.requestPayment(JSON.parse(UTF8ToString(paymentData)), function(resultData) 
      {
        console.log(resultData);
        var result = {};
        result.responseCode = resultData == null ? "failed" : resultData["payment"].getField("responseCode");
        console.log("Result payment: " + resultData);
        onPaymentFinished(result);
      });
    },

    RequestPaymentMain: function(data) 
    {
        console.log("Nutaku::requestPayment");
        console.log("before parse: " + data);
        
        var storeData = JSON.parse(UTF8ToString(data));
        console.log(storeData);

        var items = storeData.Store;
        
        var paymentItems = [];
        for( var i = 0; i < items.length; i++)
        {
            var paymentItem = this.createItem(items[i]);
            console.log("Created payment item: " + paymentItem + " by data as: " + items[i]);
            paymentItems.push(paymentItem);
        }

        console.log("Payment items: " + paymentItems);
        
        var payment = this.createPayment(paymentItems, "store description");
    }
});