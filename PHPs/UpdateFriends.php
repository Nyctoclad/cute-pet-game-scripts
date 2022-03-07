<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "UPDATE `Friend` SET `user_id`=?,`friend_user_id`=?,`pending_user_to_friend`=?,`pending_friend_to_user`=?,`mutual_friend`=?,`block_user_to_friend`=?,`block_friend_to_user`=? WHERE `user_id` = ? AND `mutual_friend` = ? OR `user_id` = ? AND `friend_user_id` = ?;";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("ddiiiiiiiii", $user_id, $friend_user_id, $pending_user_to_friend, $pending_friend_to_user, $mutual_friend, $block_user_to_friend,$block_friend_to_user, $user_id, $friend_user_id, $friend_user_id, $user_id);

    $user_id = $_POST["user_id"];
    $friend_user_id = $_POST["friend_user_id"];
    $pending_user_to_friend = $_POST["pending_user_to_friend"];
    $pending_friend_to_user = $_POST["pending_friend_to_user"];
    $mutual_friend = $_POST["mutual_friend"];
    $block_user_to_friend = $_POST["block_user_to_friend"];
    $block_friend_to_user = $_POST["block_friend_to_user"];

    $stmt->execute();
    
    $result = $stmt->get_result();

    echo $result;

    $stmt->close();
    $mysqli->close();
?>