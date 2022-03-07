<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `UserRoom` WHERE `user_id` = ?";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("d", $user_id);

    $user_id = $_POST["user_id"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["room_size"].",".$row["default_room"].",".$row["floor_material_id"].",".$row["wall_material_id"].",".$row["room_location"].",".$row["room_rotation"].",".$row["user_room_id"].";";
    }

    $stmt->close();
    $mysqli->close();
?>