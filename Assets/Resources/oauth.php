<?php
    require_once 'OAuth.php';     // HybridAuth Library
  
    // Consumer Key from Developer Site
    $consumer_key = 'H8YhSB9Xda5Q96UY';
  
    // Consumer Secret from Developer Site
    $consumer_secret = 'yRnlf9_hHFr1ExN]wzgb5g68g1MgXu-c';
  
    // OAuth Consumer
    $consumer = new OAuthConsumer($consumer_key, $consumer_secret, NULL);
  
    // Request from User
    $request = OAuthRequest::from_request(NULL, NULL, NULL);
  
    // Token from User
    $token = new OAuthToken($request -> get_parameter('oauth_token'), $request -> get_parameter('oauth_token_secret'));
  
    // Signature for this request
    $signature = $request -> get_parameter('oauth_signature');
   
    // Signature Method (HMAC-SHA1)
    // You should change this if $signature is RSA-SHA1
    $sign_method = new OAuthSignatureMethod_HMAC_SHA1();
  
    // Validate Request from User
    $valid = $sign_method -> check_signature($request, $consumer, $token, $signature);
   
    if (!$valid) {
        // Respond with an appropriate error
        exit;
    }
?>
