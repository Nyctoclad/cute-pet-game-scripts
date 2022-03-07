<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "INSERT INTO `Pet`(`user_id`, `pet_name`, `pet_id`, `pet_type`, `pet_color`,`pet_glow`,`pet_special`, `pet_face`, `pet_clothing`, `active`) VALUES (?,?,?,?,?,?,?,?,?,?)";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("dsiiiiiiii", $user_id, $pet_name, $pet_id, $pet_type, $pet_color, $pet_glow, $pet_special, $pet_face, $pet_clothing, $active);

    $user_id = $_POST["user_id"];
    $pet_name = $_POST["pet_name"];
    $pet_id = $_POST["pet_id"];
    $pet_type = $_POST["pet_type"];
    $pet_color = $_POST["pet_color"];
    $pet_glow = $_POST["pet_glow"];
    $pet_special = $_POST["pet_special"];
    $pet_face = $_POST["pet_face"];
    $pet_clothing = $_POST["pet_clothing"];
    $active = $_POST["active"];

    $stmt->execute();

    echo "Result 32 in submit new pet.";
    echo var_dump($stmt->get_result());
    
    $stmt->close();
    $mysqli->close();
?>