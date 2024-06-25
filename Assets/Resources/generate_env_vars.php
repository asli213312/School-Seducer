<?php

// Переменные окружения
$envVariables = [
    'DB_HOST' => 'localhost',
    'DB_NAME' => 'postgres',
    'DB_USER' => 'postgres',
    'DB_PASS' => 'PW_38A6ewtNcyn_VsJ'
];

// Путь к файлу .env
$envFilePath = "/var/www/html/private_vars.env";

// Создание файла .env
$envFileContent = '';
foreach ($envVariables as $key => $value) {
    $envFileContent .= "$key=$value\n";
}

file_put_contents($envFilePath, $envFileContent);

echo "Файл .env успешно создан.";