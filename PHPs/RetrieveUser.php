<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `users` WHERE `userID` = ?";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("d", $userid);

    $userid = $_POST["userid"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc()){
        echo "".$row["username"].",".$row["userID"].",".$row["active"].";";
    }

    $stmt->close();
    $mysqli->close();
?>