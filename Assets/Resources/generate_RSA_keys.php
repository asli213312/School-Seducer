<?php

// Создание конфигурации для генерации ключей
$config = array(
    "private_key_bits" => 2048,
    "private_key_type" => OPENSSL_KEYTYPE_RSA,
);

// Генерация ключевой пары
$keyPair = openssl_pkey_new($config);

// Получение открытого и приватного ключей
openssl_pkey_export($keyPair, $privateKey);
$publicKey = openssl_pkey_get_details($keyPair)['key'];

// Сохранение ключей в файлы в той же директории, где находится скрипт
file_put_contents(__DIR__ . '/private.key', $privateKey);
file_put_contents(__DIR__ . '/public.key', $publicKey);

echo "Приватный ключ сохранен в private.key\n";
echo "Открытый ключ сохранен в public.key\n";

?>