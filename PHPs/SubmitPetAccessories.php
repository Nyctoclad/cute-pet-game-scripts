<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';


    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "INSERT INTO `PetAccessory`(`user_id`, `pet_id`, `accessory_name`, `accessory_id`, `user_accessory_id`) VALUES (?,?,?,?,?)";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("disii", $user_id, $pet_id, $accessory_name, $accessory_id, $user_accessory_id);

    $user_id = $_POST["user_id"];
    $pet_id = $_POST["pet_id"];
    $accessory_name = $_POST["accessory_name"];
    $accessory_id = $_POST["accessory_id"];
    $user_accessory_id = $_POST["user_accessory_id"];


    $stmt->execute();
    
    $stmt->close();
    $mysqli->close();
?>