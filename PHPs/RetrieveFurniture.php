<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `UserFurniture` WHERE `user_id` = ?";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("d", $user_id);

    $user_id = $_POST["user_id"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["furniture_id"].",".$row["furniture_color_id"].",".$row["furniture_location"].",".$row["furniture_rotation"].",".$row["user_furniture_id"].",".$row["user_room_id"].";";
    }

    $stmt->close();
    $mysqli->close();
?>