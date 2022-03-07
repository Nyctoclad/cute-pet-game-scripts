<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `Friend` WHERE `user_id` = ? OR `friend_user_id` = ?";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("dd", $user_id, $user_id);

    $user_id = $_POST["user_id"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["user_id"].",".$row["friend_user_id"].",".$row["pending_user_to_friend"].",".$row["pending_friend_to_user"].",".$row["mutual_friend"].",".$row["block_user_to_friend"].",".$row["block_friend_to_user"].";";
    }

    $stmt->close();
    $mysqli->close();
?>