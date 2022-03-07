<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    $conn = mysqli_connect($hostname, $username, $password, $database);
    
    $sql = "SELECT `user_id` FROM User ORDER BY `user_id` DESC LIMIT 1";

    $result = $conn->query($sql);

    if ($result->num_rows > 0) {
        // output data of each row
        while($row = $result->fetch_assoc()) {
          echo "" . $row["user_id"];
        }
      } else {
        echo "0 results";
      }

    $conn->close();

?>