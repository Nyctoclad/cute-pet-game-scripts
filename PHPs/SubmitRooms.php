<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    $mysqli = mysqli_connect($hostname, $username, $password, $database);

    $user_id = $_POST["user_id"];
    $room_size = $_POST["room_size"];
    $default_room = $_POST["default_room"];
    $floor_material_id = $_POST["floor_material_id"];
    $wall_material_id = $_POST["wall_material_id"];
    $room_location = $_POST["room_location"];
    $room_rotation = $_POST["room_rotation"];
    $user_room_id = $_POST["user_room_id"];

    $sql = "SELECT * FROM `UserRoom` WHERE `user_id` =  ?";
    $stmt = $mysqli->prepare($sql);
    $stmt->bind_param("d", $user_id);
    $stmt->execute();

    $roomsresult = $stmt->get_result();
    $found = false;

    $returnmessage = $returnmessage."Getting room result. ";

    while($row = $roomsresult->fetch_assoc()){
        if($row["user_room_id"] == $user_room_id){
            $found = true;
            if($row["floor_material_id"] !== $floor_material_id || $row["wall_material_id"] !== $wall_material_id){
                $updatesql = "UPDATE `UserRoom` SET `floor_material_id`= ?, `wall_material_id`= ? WHERE `user_id`= ? AND `user_room_id`= ?";
                $updatestmt = $mysqli->prepare($updatesql);
                $updatestmt->bind_param("iidi", $floor_material_id, $wall_material_id, $user_id, $user_room_id);
                $updatestmt->execute();
                echo "Updated room entry. ";
                break;
            }
        }
    }
    
    if($found == FALSE){
        echo "No rooms matching. Adding new room... ";
        $insertsql = "INSERT INTO `UserRoom`(`user_id`, `room_size`, `default_room`, `floor_material_id`, `wall_material_id`, `room_location`, `room_rotation`, `user_room_id`) VALUES (?,?,?,?,?,?,?,?)";
        $insertstmt = $mysqli->prepare($insertsql);
        echo "user_id is now ".$user_id." and user_room_id is now ".$user_room_id.". ";
        $insertstmt->bind_param("diiiissi", $user_id, $room_size, $default_room, $floor_material_id, $wall_material_id, $room_location, $room_rotation, $user_room_id);
        
        $insertstmt->execute();
        echo "Preparing statement: ".var_dump($insertstmt);
        echo "Closing statement. ";
        $message = var_dump($insertstmt->get_result());
        echo $message;
        echo " Added room entry.";
    }    
    
    $stmt->close();
    $mysqli->close();
?>