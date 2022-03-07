<?php
    $hostname = 'localhost';
    $username = 'hoinen';
    $password = 'ym835d8XTqmB50xU';
    $database = 'cpg';

    $mysqli_conection = mysqli_connect($hostname, $username, $password, $database);
    if($mysqli_conection){
        echo "Connected!";
    } 
    if(!$mysqli_conection){
        echo "Not able to connect!";
    }

    
?>