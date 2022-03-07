<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "INSERT INTO `User`(`username`, `user_id`, `active`) VALUES (?,?,?)";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("sdi", $username, $user_id, $active);

    $username = $_POST["username"];
    $user_id = $_POST["user_id"];
    $active = $_POST["active"];

    $stmt->execute();
    
    $stmt->close();
    $mysqli->close();
?>