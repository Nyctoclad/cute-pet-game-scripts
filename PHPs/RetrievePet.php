<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `Pet` WHERE `user_id` = ? ORDER BY `pet_id` ASC";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("d", $user_id);

    $user_id = $_POST["user_id"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["pet_name"].",".$row["pet_id"].",".$row["pet_type"].",".$row["pet_color"].",".$row["pet_glow"].",".$row["pet_special"].",".$row["pet_face"].",".$row["pet_clothing"].",".$row["active"].";";
    }

    $stmt->close();
    $mysqli->close();
?>