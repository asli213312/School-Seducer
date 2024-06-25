<?php
header('Content-Type: application/json');

// Ваши данные или что-то другое
$data = array('message' => 'Hello from PHP!');

// Возвращаем данные в формате JSON
echo json_encode($data);
?>