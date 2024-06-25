<?php

if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Выводим сообщение в консоль браузера
    echo '<script>console.log("Script tokenGenerator.php invoked!")</script>';

// Параметры подключения к базе данных
$host = 'localhost'; // адрес сервера 
$name = 'postgres'; // имя базы данных
$user = 'postgres'; // имя пользователя
$pass = 'PW_38A6ewtNcyn_VsJ'; // пароль

// Создание строки DSN
$dsn = "pgsql:host=$host;port=$port;dbname=$db;user=$user;password=$pass";
$options = [
    PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    PDO::ATTR_EMULATE_PREPARES   => false,
];

try {
    // Подключение к базе данных
    $pdo = new PDO($dsn, $user, $pass, $options);
    
    // Получаем ID игрока из POST-запроса
    $playerId = isset($_POST['id']) ? (int)$_POST['id'] : 0;
    
    // Проверяем, есть ли уже токен для этого игрока
    $stmt = $pdo->prepare("SELECT token FROM players WHERE id = ?");
    $stmt->execute([$playerId]);
    $token = $stmt->fetchColumn();
    
    if (!$token) {
        // Если токена нет, создаем новый
        $token = bin2hex(random_bytes(16)); // Генерируем уникальный токен
        $stmt = $pdo->prepare("INSERT INTO players (id, token) VALUES (?, ?) ON CONFLICT (id) DO UPDATE SET token = EXCLUDED.token");
        $stmt->execute([$playerId, $token]);
    }
    
    // Возвращаем токен и ID игрока в формате JSON
    echo json_encode(['id' => $playerId, 'token' => $token]);
} catch (\PDOException $e) {
    // В случае ошибки подключения или запроса
    echo "Ошибка базы данных: " . $e->getMessage();
    exit;
}

?>