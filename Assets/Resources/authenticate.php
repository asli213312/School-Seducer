<?php

echo '<script>console.log("Script authenticate.php invoked!")</script>';

// Эти функции для вычисления подписи и базовой строки уже должны быть в вашем коде
function computeBaseString($httpMethod, $url, $parameters) {
    // URL-кодируем URL-адрес
    $encodedUrl = rawurlencode($url);

    // Сортировка параметров по имени и создание строки параметров
    ksort($parameters);
    $encodedParameters = array();
    foreach ($parameters as $key => $value) {
        $encodedParameters[] = rawurlencode($key) . '=' . rawurlencode($value);
    }
    $parameterString = implode('&', $encodedParameters);

    // Формирование базовой строки
    $baseString = $httpMethod . '&' . $encodedUrl . '&' . rawurlencode($parameterString);

    return $baseString;
}

function computeSignature($baseString, $consumerSecret, $tokenSecret = '') {
    // Формирование хэш-ключа
    $hashKey = rawurlencode($consumerSecret) . '&' . rawurlencode($tokenSecret);

    // Вычисление подписи с использованием HMAC-SHA1
    $signature = base64_encode(hash_hmac('sha1', $baseString, $hashKey, true));

    return $signature;
}

function collectOAuthParameters() {
    $parameters = [];
    foreach ($_POST as $key => $value) {
        if (strpos($key, 'oauth_') === 0) {
            $parameters[$key] = $value;
        }
    }
    // Добавьте любые другие параметры, если нужно
    return $parameters;
}

if ($_SERVER["REQUEST_METHOD"] == "POST") {
    echo '<script>console.log("Script authenticate.php invoked!")</script>';

    // Сбор параметров OAuth
    $oauthParameters = collectOAuthParameters();

    // Предположим, что URL вашего скрипта и метод запроса следующие
    $url = 'https://fuckfest.college/scripts/authenticate.php';
    $httpMethod = 'POST';

    // Заранее известный consumerSecret
    $consumerSecret = 'yRnlf9_hHFr1ExN]wzgb5g68g1MgXu-c';
    $tokenSecret = '';

    // Вычисляем базовую строку и подпись
    $baseString = computeBaseString($httpMethod, $url, $oauthParameters);
    $computedSignature = computeSignature($baseString, $consumerSecret, $tokenSecret);

    // Проверка подписи
    if (isset($oauthParameters['oauth_signature']) && $computedSignature == base64_decode($oauthParameters['oauth_signature'])) {
        echo '<script>console.log("OAuth signature validation successful!")</script>';

        // Теперь, когда подпись OAuth проверена, мы можем сгенерировать и вернуть токен
        // Параметры подключения к базе данных
        $host = 'localhost'; 
        $dbname = 'postgres'; // Исправьте на имя вашей БД
        $user = 'postgres'; 
        $pass = 'PW_38A6ewtNcyn_VsJ'; 

        // Создание строки DSN
        $dsn = "pgsql:host=$host;dbname=$dbname";
        $options = [
            PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES   => false,
        ];

        try {
            $pdo = new PDO($dsn, $user, $pass, $options);

            $playerId = isset($_POST['id']) ? (int)$_POST['id'] : 0;

            $stmt = $pdo->prepare("SELECT token FROM players WHERE id = ?");
            $stmt->execute([$playerId]);
            $token = $stmt->fetchColumn();

            if (!$token) {
                $token = bin2hex(random_bytes(16));
                $stmt = $pdo->prepare("INSERT INTO players (id, token) VALUES (?, ?) ON CONFLICT (id) DO UPDATE SET token = EXCLUDED.token");
                $stmt->execute([$playerId, $token]);
            }

            echo json_encode(['id' => $playerId, 'token' => $token]);
        } catch (\PDOException $e) {
            echo '<script>console.log("Database error: ' . $e->getMessage() . '")</script>';
            echo "Database error: " . $e->getMessage();
            exit;
        }
    } else {
        echo '<script>console.log("OAuth signature validation failed!")</script>';
        http_response_code(401); // Unauthorized
        echo "OAuth signature validation failed.";
        exit;
    }
}