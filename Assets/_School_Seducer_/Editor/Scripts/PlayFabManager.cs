using UnityEngine;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _School_Seducer_.Editor.Scripts.UI.Shop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nutaku.Unity;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;

namespace _School_Seducer_.Editor.Scripts
{
    public class PlayFabManager : MonoBehaviour
    {
        public static bool IsPositivePayment;

        public string PlayFabId;
        public string SessionTicket;

        [SerializeField] private UnityEvent paymentFailed;
        [SerializeField] private UnityEvent paymentSuccess;

        private const string STORE_ID = "myStore";
        private const string CATALOG_VERSION = "catalogVer1";

        private RawResult _currentRawResult;
        private Payment _currentResponsePayment;

        [DllImport("__Internal")]
        private static extern void RequestPayment(string paymentData, string sessionTicket, string playfabId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PaymentCallback(string result);
        
        public void Awake()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
                /*
                Please change the titleId below to your own titleId from PlayFab Game Manager.
                If you have already set the value in the Editor Extensions, this can be skipped.
                */
                PlayFabSettings.staticSettings.TitleId = "98D64";
            }
            var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        
        public void OnPaymentFinished(string result) 
        {
            Debug.Log("OnPaymentFinished invoked function");

            JObject response = JObject.Parse(result);
            Debug.Log("Response after payment: " + response);

            // Проверка наличия кода ответа
            if (response.ContainsKey("responseCode"))
            {
                // Получение значения кода ответа
                string responseCode = (string)response["responseCode"];

                // Проверка кода ответа
                if (responseCode == "ok" || responseCode == "200")
                {
                    // Код 200 - успех
                    Debug.Log("Payment success!");
                    IsPositivePayment = true;

                    paymentSuccess?.Invoke();
                }
                else
                {
                    // Код ответа отличный от 200 - ошибка
                    Debug.LogError("Payment failed with response code: " + responseCode);
                    paymentFailed?.Invoke();
                }
            }
            else
            {
                // Ошибка: нет кода ответа в JSON
                Debug.LogError("Invalid response format: 'responseCode' field not found.");
                paymentFailed?.Invoke();
            }
        }

        public void GetStoreItems(IShopItemDataBase shopItem)
        {
            if (shopItem != null)
            {
                //Debug.Log("Bought item: " + shopItem.Id);
            }
            
            var request = new GetStoreItemsRequest
            {
                StoreId = STORE_ID,
                CatalogVersion = CATALOG_VERSION
            };
            
            PlayFabClientAPI.GetStoreItems(request, resultStore => 
            {
                List<StoreItem> storeItems = new List<StoreItem>();
                
                foreach (var item in resultStore.Store)
                {
                    if (item.ItemId == shopItem.Id)
                    {
                        storeItems.Add(item);
                        break;
                    }
                }
                
                var jsonResult = JsonConvert.SerializeObject(storeItems);
                Debug.Log("JSON result store items: " + JsonConvert.SerializeObject(storeItems));

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    RequestPayment(jsonResult, SessionTicket, PlayFabId);
                    return;
                }
                
                var requestCatalog = new GetCatalogItemsRequest { CatalogVersion = CATALOG_VERSION };
                PlayFabClientAPI.GetCatalogItems(requestCatalog, resultCatalog =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        StartCoroutine(ProcessPaymentAndroid());
                        IEnumerator ProcessPaymentAndroid()
                        {
                            RequestPaymentAndroid(resultStore.Store, resultCatalog.Catalog, shopItem);
                            yield return new WaitUntil(() => _currentResponsePayment != null);
                            try
                            {
                                if (_currentResponsePayment == null) Debug.LogError("OpenPaymentView failed: requestPayment is null");

                                SdkPlugin.OpenPaymentView(_currentResponsePayment, PaymentResult);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("OpenPaymentView failed: "+ e.Message);
                            }

                            void PaymentResult(WebViewEvent paymentResult)
                            {
                                try
                                {
                                    Debug.Log("Event: " + paymentResult.kind);
                                    Debug.Log("Result: " + paymentResult.value);
                                    Debug.Log("Message: " + paymentResult.message);
                                    switch (paymentResult.kind)
                                    {
                                        case WebViewEventKind.Succeeded:
                                            PaymentSucceeded(_currentResponsePayment, _currentRawResult);
                                            break;

                                        case WebViewEventKind.Failed:
                                            Debug.Log("Error during purchase");
                                            paymentFailed?.Invoke();
                                            break;

                                        case WebViewEventKind.Cancelled:
                                            Debug.Log("User cancelled the purchase");
                                            paymentFailed?.Invoke();
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError("Error during purchase: "+ ex.Message);

                                    paymentFailed?.Invoke();
                                }
                            }
                        }
                    }

                }, error => Debug.LogError(error.GenerateErrorReport()));
            }, error => Debug.LogError("Error fetching store items: " + error.GenerateErrorReport()));
        }

        private void PaymentSucceeded(Payment payment, RawResult rawResult) 
        {
            try
            {
                RestApiHelper.GetPayment(SdkPlugin.loginInfo.userId, payment.paymentId, this, TestBatchGetPaymentCallback);
 
                var result = RestApi.HandleRequestCallback<Payment>(rawResult);
                Payment responsePayment = result.GetFirstEntry();
                Debug.Log("Payment ID: " + responsePayment.paymentId);
                Debug.Log("Status: " + responsePayment.status);
                Debug.Log("Ordered Date and time: " + responsePayment.orderedTime);
                // more fields are available here. For example: responsePayment.paymentItems, which is List<PaymentItem>
            }
            catch (Exception ex)
            {
                // error handling
            }
        }
        
        private void TestBatchGetPaymentCallback(RawResult rawResultCallback)
        {
            try
            {
                if ((rawResultCallback.statusCode > 199) && (rawResultCallback.statusCode < 300))
                {
                    var result = RestApi.HandleRequestCallback<Payment>(rawResultCallback);
                    Payment responsePayment = result.GetFirstEntry();
                    Debug.Log("Payment ID: " + responsePayment.paymentId);
                    Debug.Log("Status: " + responsePayment.status);
                    Debug.Log("Ordered Date and time: " + responsePayment.orderedTime);
                    Debug.Log("Payment success!");

                    IsPositivePayment = true;

                    paymentSuccess?.Invoke();
                    // more fields are available here. For example: responsePayment.paymentItems, which is List<PaymentItem>
                }
                else
                {
                    Debug.LogError("BatchGetPayment Failure");
                    Debug.LogError("Http Status Code: " + (int)rawResultCallback.statusCode);
                    Debug.LogError("Http Status Message: " + Encoding.UTF8.GetString(rawResultCallback.body));

                    paymentFailed?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("BatchGetPayment Failure");
                Debug.Log("Exception: "+ ex.Message);
            }
        }

        private void RequestPaymentAndroid(List<StoreItem> items, List<CatalogItem> catalogItems, IShopItemDataBase shopItem)
        {
            Payment responsedPayment = null;
            Payment requestPayment = new Payment
            {
                callbackUrl = "https://example.com/callback",
                finishPageUrl = "dummy", // this particular url is not used for APK payment flow, but it should be set with a dummy value
                message = "Test payment", // optional. we recommend leaving this blank and using the Name and Description fields from the item definition
                catalogVersion = "catalogVer1",
                storeId = "myStore",
                sessionTicket = SessionTicket,
                playFabId = PlayFabId
            };

            for (int i = 0; i < items.Count && i < catalogItems.Count; i++)
            {
                if (catalogItems[i].ItemId != shopItem.Id) continue;

                Debug.Log("Selected item: " + catalogItems[i].ItemId);
                Debug.Log("DisplayName: " + catalogItems[i].DisplayName);

                PaymentItem apiItem = new PaymentItem();
                apiItem.itemId = items[i].ItemId;
                apiItem.itemName = catalogItems[i].DisplayName;
                apiItem.unitPrice = (int)items[i].VirtualCurrencyPrices["NG"];
                apiItem.imageUrl = "https://college-fuck-fest.nyc3.cdn.digitaloceanspaces.com/CFF/example_item.png"; // You must use https, because Android started blocking http content in WebViews
                apiItem.description = "Description";
                requestPayment.paymentItems.Add(apiItem);
            }

            // the current system supports multiple item types in the same payment, but this will be deprecated. Please only use 1 item object per payment.
            
            Debug.Log("Request payment items count: " + requestPayment.paymentItems.Count);
 
            try
            {
                RestApiHelper.PostPayment(requestPayment, this, TestRequestPostPaymentCallback);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error posting payment: "+ ex.Message);
            }
            
            Debug.Log("Request payment status: " + requestPayment.status);

// callback method implementation:
            void TestRequestPostPaymentCallback(RawResult rawResult)
            {
                Debug.Log("Status after POST payment: " + rawResult.statusCode);
                _currentRawResult = rawResult;
                try
                {
                    if (rawResult.statusCode is > 199 and < 300)
                    {
                        var result = RestApi.HandleRequestCallback<Payment>(rawResult);
                        Payment responsePayment = result.GetFirstEntry(); //this is the Payment object which you should keep to be used for payment completion later
                        Debug.Log("Payment ID: " + responsePayment.paymentId);
                        Debug.Log("Status: " + responsePayment.status);
                        Debug.Log("Ordered Date and time: " + responsePayment.orderedTime);
                        Debug.Log("Response payment: " + responsePayment);
                        
                        _currentResponsePayment = responsePayment;
                        
                        if (_currentResponsePayment != null)
                            Debug.Log("Current response payment: "+ _currentResponsePayment);
                    }
                    else
                    {
                        Debug.Log("Http Status Code: " + rawResult.statusCode);
                        Debug.Log("Http Status Message: " + Encoding.UTF8.GetString(rawResult.body));

                        paymentFailed?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Payment is fail, stacktrace: " + ex.StackTrace +" | " + ex.Message);
                }
            }

            Debug.Log("Request payment after request: " + requestPayment.paymentId);
        }
        
        private void OnLoginSuccess(LoginResult result)
        {
            PlayFabId = result.PlayFabId;
            SessionTicket = result.SessionTicket;
        }
    
        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}