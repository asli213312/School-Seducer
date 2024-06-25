"use strict"; // this line probably needs to be at the start of your script file. If you paste this code at the end of your file, you might want to move this line to the top
 
//this is your custom secret key that you entered in Nutaku developer portal. Don't confuse with PlayFab API SecretKey! Please change it to something of your own, don't leave it as MyCustomKey
const customKey = "Y4PU44AOTY3H9EMIR41J9MO139XISW5QGPQN4D5QCQAP7D8TEQ";
 
/*
NutakuGoldCheck
This handler checks that the Item ID specified exists in the specified catalog or store, and that the Nutaku Gold cost is accurate.
*/
handlers.NutakuGoldCheck = function (args, context) {
    // checks the secret key to see if it is a valid S2S call from Nutaku server
    if (args.customKey != customKey)
        return { "status": "failure", "error": "WrongSecretKey" };
 
    // checks if the session ticket is valid and belongs to the specified PlayFab ID
    var playerTicketInfo = {};
    try { playerTicketInfo = server.AuthenticateSessionTicket({ SessionTicket: args.sessionTicket }); }
    catch (ex) { return { "status": "failure", "error": "SessionTicketLookupFailure", "PlayFabError": ex.apiErrorInfo.apiError.error }; }
    if (playerTicketInfo.hasOwnProperty('UserInfo')) {
        if (!playerTicketInfo.UserInfo.hasOwnProperty('PlayFabId') || playerTicketInfo.UserInfo.PlayFabId != args.playFabId)
            return { "status": "failure", "error": "PlayFabIdMismatch" };
    }
 
    var storeItem = null;
    if (args.useStore) {
        // if the game is configured in nutaku to use Stores, it loops through the specified store of the specified catalog to look for the item id
        var storeInfo = {};
        try {
            var storeRequest = {
                PlayFabId: args.playFabId,
                CatalogVersion: args.catalogVersion,
                StoreId: args.storeId
            };
            storeInfo = server.GetStoreItems(storeRequest);
        } catch (ex) { return { "status": "failure", "error": "GetStoreFailure", "PlayFabError": ex.apiErrorInfo.apiError.error }; }
         
        for (var c = 0; c < storeInfo.Store.length; c++)
            if (args.itemId === storeInfo.Store[c].ItemId) {
                storeItem = storeInfo.Store[c];
                break;
            }
    }
    else {
        // if the game is configured in nutaku to use Catalog only, it loops through the specified catalog to look for the item id
        var catalogInfo = {};
        try {
            var catalogRequest = {
                PlayFabId: args.playFabId,
                CatalogVersion: args.catalogVersion
            };
            catalogInfo = server.GetCatalogItems(catalogRequest);
        } catch (ex) { return { "status": "failure", "error": "GetStoreFailure", "PlayFabError": ex.apiErrorInfo.apiError.error }; }
         
        for (var c = 0; c < catalogInfo.Catalog.length; c++)
            if (args.itemId === catalogInfo.Catalog[c].ItemId) {
                storeItem = catalogInfo.Catalog[c];
                break;
            }
    }
     
    if (!storeItem) {
        // if item was not found, log an event and respond back to Nutaku accordingly
        var eventRequest = {
            PlayFabId: args.playFabId,
            EventName: "NutakuGoldCheck_ItemIdNotFound",
            Body: {
                CatalogVersion: args.catalogVersion,
                StoreId: args.storeId,
                PlayFabId: args.playFabId,
                ItemId: args.itemId,
                Price: args.price
            }
        };
        server.WritePlayerEvent(eventRequest);
        return { "status": "failure", "error": "ItemIdNotFound", "PlayFabError": null };
    }
     
    if (storeItem.VirtualCurrencyPrices["NG"] != args.price) {
        // if price from Nutaku doesn't match price in the store/catalog, and respond back to Nutaku accordingly
        var eventRequest = {
            PlayFabId: args.playFabId,
            EventName: "NutakuGoldCheck_PriceMismatch",
            Body: {
                CatalogVersion: args.catalogVersion,
                StoreId: args.storeId,
                PlayFabId: args.playFabId,
                ItemId: args.itemId,
                Price: args.price
            }
        };
        server.WritePlayerEvent(eventRequest);
        return { "status": "failure", "error": "PriceMismatch", "PlayFabError": null };
    }
     
    // everything was good, reply back to Nutaku saying that the item is valid and purchase can continue
    return { "status": "success" };
};
 
/*
NutakuGoldFinalize
This handler requests that the PlayFab server grants the item from the catalog to the user.
If it works successfully, Nutaku will consume the user's gold upon status=success result
*/
handlers.NutakuGoldFinalize = function (args, context) {
    // checks the secret key to see if it is a valid S2S call from Nutaku server
    if (args.customKey != customKey)
        return { "status": "failure", "error": "WrongSecretKey" };
 
    // checks if the session ticket is valid and belongs to the specified PlayFab ID
    var playerTicketInfo = {};
    try { playerTicketInfo = server.AuthenticateSessionTicket({ SessionTicket: args.sessionTicket }); }
    catch (ex) { return { "status": "failure", "error": "SessionTicketLookupFailure", "PlayFabError": ex.apiErrorInfo.apiError.error }; }
    if (playerTicketInfo.hasOwnProperty('UserInfo')) {
        if (!playerTicketInfo.UserInfo.hasOwnProperty('PlayFabId') || playerTicketInfo.UserInfo.PlayFabId != args.playFabId)
            return { "status": "failure", "error": "PlayFabIdMismatch" };
    }
 
    // Prepare and trigger item grant to the user
    var grantRequest = {
        PlayFabId: args.playFabId,
        CatalogVersion: args.catalogVersion,
        ItemIds: [ args.itemId ]
    };
    try {
        // if you want to add any extra logic before or after server.GrantItemsToUser(), make sure you properly handle reverts for the extra stuff in the catch block
        server.GrantItemsToUser(grantRequest);
    }
    catch (ex) {
        // something unexpected went wrong on PlayFab servers. Nutaku will not spend the user's gold.
        return { "status": "failure", "error": "GrantItemFailure", "PlayFabError": ex.apiErrorInfo.apiError.error };
    }
     
    try {
        server.AddUserVirtualCurrency({ PlayFabId: args.playFabId, VirtualCurrency: "NG", Amount: args.price });
    } catch (ex) {}
 
    // log an event for the successful grant of the item
    try {
        var eventRequest = {
            PlayFabId: args.playFabId,
            EventName: "NutakuGoldFinalize_Success",
            Body: {
                CatalogVersion: args.catalogVersion,
                StoreId: args.storeId,
                ItemIdRequesed: args.itemId,
                Price: args.price
            }
        };
        server.WritePlayerEvent(eventRequest);
    } catch (ex) { var tmp = "the item was granted but event logging failed. Ignore and move on"; }
 
    // reply back to Nutaku, telling it that all was ok and gold can be subtracted. Without this reply, Nutaku Gold will NOT be consumed
    return { "status": "success" };
};