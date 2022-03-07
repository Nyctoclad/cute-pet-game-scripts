
CREATE TABLE `Animal` (
  `animal_id` int NOT NULL,
  `animal_name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `color_id` int NOT NULL,
  `face_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



CREATE TABLE `Clothing` (
  `clothing_name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `clothing_id` int NOT NULL,
  `color_id` int NOT NULL,
  `color_name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `Friend` (
  `user_id` float NOT NULL,
  `friend_user_id` float NOT NULL,
  `pending_user_to_friend` int NOT NULL,
  `pending_friend_to_user` int NOT NULL,
  `mutual_friend` int NOT NULL,
  `block_user_to_friend` int NOT NULL,
  `block_friend_to_user` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `Pet` (
  `user_id` double NOT NULL,
  `pet_name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `pet_id` int NOT NULL,
  `pet_type` int NOT NULL,
  `pet_color` int NOT NULL,
  `pet_glow` int NOT NULL,
  `pet_special` int NOT NULL,
  `pet_face` int NOT NULL,
  `pet_clothing` int NOT NULL,
  `active` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `PetAccessory` (
  `user_id` float NOT NULL,
  `pet_id` int NOT NULL,
  `accessory_name` int NOT NULL,
  `accessory_id` int NOT NULL,
  `user_accessory_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `User` (
  `username` varchar(20) NOT NULL,
  `user_id` float NOT NULL,
  `active` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `UserFurniture` (
  `user_id` float NOT NULL,
  `user_furniture_id` float NOT NULL,
  `furniture_id` int NOT NULL,
  `furniture_color_id` int NOT NULL,
  `furniture_location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `furniture_rotation` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_room_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `UserRoom` (
  `user_id` float NOT NULL,
  `room_size` int NOT NULL,
  `default_room` int NOT NULL,
  `floor_material_id` int NOT NULL,
  `wall_material_id` int NOT NULL,
  `room_location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `room_rotation` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_room_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

