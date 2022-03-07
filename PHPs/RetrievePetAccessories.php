<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `PetAccessory` WHERE `user_id` = ? AND `pet_id` = ? ORDER BY `pet_id` ASC";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("di", $user_id, $pet_id);

    $user_id = $_POST["user_id"];
    $pet_id = $_POST["pet_id"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["accessory_name"].",".$row["accessory_id"].",".$row["user_accessory_id"].";";
    }

    $stmt->close();
    $mysqli->close();
?>