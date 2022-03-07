<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';
    

    $mysqli = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT * FROM `UserFurniture` WHERE `user_id` =  ?";

    $stmt = $mysqli->prepare($sql);

    $stmt->bind_param("d", $user_id);
    
    $user_id = $_POST["user_id"];
    $user_furniture_id = $_POST["user_furniture_id"];
    $furniture_id = $_POST["furniture_id"];
    $furniture_color_id = $_POST["furniture_color_id"];
    $furniture_location = $_POST["furniture_location"];
    $furniture_rotation = $_POST["furniture_rotation"];
    $user_room_id = $_POST["user_room_id"];

    $stmt->execute();
    
    //If everything is the same, do nothing. If something is different, do something. If the unique ID does not exist, put it in as a new

    $furnitureresult = $stmt->get_result();
    $found = false;
    $changed = false;

    while($row = $furnitureresult->fetch_assoc()){

        if($row["user_furniture_id"] == $user_furniture_id){
            $found = true;
            if($row["furniture_color_id"] !== $furniture_color_id || $row["furniture_location"] !== $furniture_location || $row["furniture_rotation"] !== $furniture_rotation || $row["user_room_id"] !== $user_room_id){
                $updatesql = "UPDATE `UserFurniture` SET `furniture_color_id`= ?,`furniture_location`= ?,`furniture_rotation`= ?,`user_room_id`=? WHERE `user_id` = ? AND `user_furniture_id` = ?";
                $updatestmt = $mysqli->prepare($updatesql);
                $updatestmt->bind_param("issidd", $furniture_color_id, $furniture_location, $furniture_rotation, $user_room_id, $user_id, $user_furniture_id);
                $updatestmt->execute();
                $changed = true;
                echo "Updated entry ".$furniture_location;
                break;
            }
        }
    }

    if($found == FALSE){
        //Add new
        $insertsql = "INSERT INTO `UserFurniture`(`user_id`, `user_furniture_id`, `furniture_id`, `furniture_color_id`, `furniture_location`, `furniture_rotation`,`user_room_id`) VALUES (?,?,?,?,?,?,?)";
        $insertstmt = $mysqli->prepare($insertsql);
        $insertstmt->bind_param("ddiissi", $user_id, $user_furniture_id, $furniture_id, $furniture_color_id, $furniture_location, $furniture_rotation, $user_room_id);
        $insertstmt->execute();
        echo "Added new entry.";
    }
    if($found == TRUE && $changed == FALSE){
        echo "No changes needed";
    }

    $stmt->close();
    $mysqli->close();
?>