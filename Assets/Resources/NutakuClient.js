var Nutaku = 
{
	playFabId: 0,
  	sessionTicket: 0, 

	showCreateUser : function()
    {
        var params = {};
 
        params[nutaku.GuestRequestFields.VERSION] = 1; // specify the API version (optional).
         
        var guest = opensocial.newGuestSignUp(params); // convert the request value to a guest object.
         
        // the Guest SignUp modal is displayed when calling "opensocial.requestGuestSignUp".
        opensocial.requestGuestSignUp(guest, function(response) {
            // Callback processing is performed once the user action on the signup modal is completed.   
            // However, if the user selects "Sign Up", the user will be directed to the signup page, thus the callback processing is not executed.
            if (!response.hadError()) {
                var responseCode = guest.getField(nutaku.Guest.Field.RESPONSE_CODE);
            }
            else {
            //error handling
            }
        });
    },
    
	retrieveUser : function(callback) 
	{
		console.log("Nutaku::retrieveUser");
	
		var params = {};
		params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] = [
			nutaku.Person.Field.USER_TYPE,
			nutaku.Person.Field.GRADE,
			opensocial.Person.Field.NICKNAME,
			opensocial.Person.Field.GENDER,
			opensocial.Person.Field.LANGUAGES_SPOKEN
		];
		
		var req = opensocial.newDataRequest();
		req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER, params), "viewer");
		req.send(function(response) 
		{
			if (response.hadError()) 
			{
				console.log("NutakuClient::Error " + "Could not fetch person request");
			} 
			else 
			{
				console.log(response);
				var item = response.get("viewer");
				if (item.hadError()) 
				{
					console.log("NutakuClient::Error " + "Could not fetch viewer request");
				} 
				else 
				{
					var viewer = item.getData();
					var id = viewer.getId();
					var userGrade = viewer.getField(nutaku.Person.Field.GRADE);					
					var nickname = viewer.getDisplayName();
					var userType = viewer.getField(nutaku.Person.Field.USER_TYPE);					
					var gender = viewer.getField(opensocial.Person.Field.GENDER);
					var language = viewer.getField(opensocial.Person.Field.LANGUAGES_SPOKEN);
					console.log(viewer);
					
					var result = {};
					result.id = id;
					result.nickname = nickname;
					result.gender = gender;
					result.language = language;
					result.grade = userGrade;
					
					callback(result);
				}
			}
		});
	},
	
	sendMessage : function(id)
	{
		var params = {};
		params[opensocial.Message.Field.TITLE] = "Hello!";
		
		var appParams = { request_type : "message"};
		var escaped = gadgets.io.encodeValues({appParams: gadgets.json.stringify(appParams)});
		
		var pcUrl = {}; //this one is for users viewing the site on PC browser
		pcUrl[nutaku.Url.Field.VALUE] = "?appParams=" + escaped;
		pcUrl[opensocial.Url.Field.TYPE] = nutaku.Message.UrlType.CANVAS;
		
		var spUrl = {}; //this one is for users viewing the site on a SmartPhone
		spUrl[nutaku.Url.Field.VALUE] = "https://www.example.net/games/example-game/play/";
		spUrl[opensocial.Url.Field.TYPE] = nutaku.Message.UrlType.TOUCH;
		
		params[opensocial.Message.Field.URLS] = [pcUrl, spUrl];

		var body = "Hello! I need help defeating the boss";
		var message = opensocial.newMessage(body, params);
		var recipients = [id]; //can also be a comma-separated string
		
		opensocial.requestSendMessage(recipients, message, function(response) 
		{
		 if (response.hadError()) 
		 {
			console.log("ERROR SENDING MESSAGE");
		 } else {
			console.log("SUCCESS SENDING MESSAGE");
		 }

		});
	},

	createItem : function(data)
	{
		console.log("Nutaku::createItem");

		function guid() {
		  function s4() {
		    return Math.floor((1 + Math.random()) * 0x10000)
		      .toString(16)
		      .substring(1);
		  }
		  return s4() + s4() + '-' + s4();
		}
	
		console.log("Create item data: " + data);
			
		var itemParams = {};
		
		//if (data.id == null || data.id == "" || data.id == undefined) data.id = guid();
		
		if (data.name == null || data.name == "") data.name = "Gift";
		if (data.description == null || data.name == "") data.description = "Gift";
		
		itemParams[opensocial.BillingItem.Field.SKU_ID] = data.ItemId;
		itemParams[opensocial.BillingItem.Field.PRICE] = data.VirtualCurrencyPrices['NG'];
		itemParams[opensocial.BillingItem.Field.COUNT] = 1;
		itemParams[opensocial.BillingItem.Field.DESCRIPTION] = data.description;
		itemParams[nutaku.BillingItem.Field.NAME] = data.ItemId;
		itemParams[nutaku.BillingItem.Field.IMAGE_URL] = "https://college-fuck-fest.nyc3.cdn.digitaloceanspaces.com/CFF/example_item.png";
		
		var item = opensocial.newBillingItem(itemParams);
		return item;
	},
	
	createPayment : function(items, description)
	{
		console.log("Nutaku::createPayment");
		console.log(items);
	
		var params = {};
		params[opensocial.Payment.Field.ITEMS]   = items;
		params[opensocial.Payment.Field.MESSAGE] = description;
		params[opensocial.Payment.Field.PAYMENT_TYPE]  = opensocial.Payment.PaymentType.PAYMENT;
		//params[nutaku.GuestRequestFields.VERSION] = 3;

		params['playFabId'] = Nutaku.playFabId;
		params['sessionTicket'] = Nutaku.sessionTicket;
		params['catalogVersion'] = 'catalogVer1';
		params['storeId'] = 'myStore';
		
		var payment = opensocial.newPayment(params);	
		return payment;
	},
	
	createPaymentRequest : function(payment, callback)
	{
		console.log("Nutaku::createPaymentRequest");
		console.log(payment);
			
		opensocial.requestPayment(payment, function(response) 
		{
			if (response.hadError()) 
			{
				console.log("NutakuClient::Error " + "Could not create payment request");
				console.log(response);
				callback(null);
			} 
			else 
			{
				var result = {};
				result.payment = response.getData();
				result.paymentId = payment.getField(nutaku.Payment.Field.PAYMENT_ID);
				
				callback(result);
			}
		});
	},
	
	requestPayment : function(data, callback)
	{
		console.log("Nutaku::requestPayment");
    	console.log("after parse: " + data);

    	var items = data;
		
		var paymentItems = [];
		for( var i = 0; i < items.length; i++)
		{
			var paymentItem = this.createItem(items[i]);
			console.log("Created payment item: " + paymentItem + " by data as: " + items[i]);
			paymentItems.push(paymentItem);
		}

		console.log("Payment items: " + paymentItems);
		
		var payment = this.createPayment(paymentItems, "store description");
		
		this.createPaymentRequest(payment, callback);
	}
}