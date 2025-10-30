-- service
INSERT INTO service (id, name, icon)
VALUES (1, 'FFXIV Global', '🇺🇳'),
       (2, 'FFXIV Korea', '🇰🇷'),
       (3, 'FFXIV China', '🇨🇳');

-- repository
INSERT INTO repository (id, name, description, slug, service_id)
VALUES (1, 'ffxivneo/win32/release/boot', 'FFXIV Global/JP - Retail - Boot - Win32', '2b5cbc63', 1),
       (2, 'ffxivneo/win32/release/game', 'FFXIV Global/JP - Retail - Base Game - Win32', '4e9a232b', 1),
       (3, 'ffxivneo/win32/release/ex1', 'FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32', '6b936f08', 1),
       (4, 'ffxivneo/win32/release/ex2', 'FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32', 'f29a3eb2', 1),
       (5, 'ffxivneo/win32/release/ex3', 'FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32', '859d0e24', 1),
       (6, 'ffxivneo/win32/release/ex4', 'FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32', '1bf99b87', 1),
       (7, 'actoz/win32/release_ko/game', 'FFXIV Korea - Retail - Base Game - Win32', 'de199059', 2),
       (8, 'actoz/win32/release_ko/ex1', 'FFXIV Korea - Retail - ex1 (Heavensward) - Win32', '573d8c07', 2),
       (9, 'actoz/win32/release_ko/ex2', 'FFXIV Korea - Retail - ex2 (Stormblood) - Win32', 'ce34ddbd', 2),
       (10, 'actoz/win32/release_ko/ex3', 'FFXIV Korea - Retail - ex3 (Shadowbringers) - Win32', 'b933ed2b', 2),
       (11, 'actoz/win32/release_ko/ex4', 'FFXIV Korea - Retail - ex4 (Endwalker) - Win32', '27577888', 2),
       (12, 'shanda/win32/release_chs/game', 'FFXIV China - Retail - Base Game - Win32', 'c38effbc', 3),
       (13, 'shanda/win32/release_chs/ex1', 'FFXIV China - Retail - ex1 (Heavensward) - Win32', '77420d17', 3),
       (14, 'shanda/win32/release_chs/ex2', 'FFXIV China - Retail - ex2 (Stormblood) - Win32', 'ee4b5cad', 3),
       (15, 'shanda/win32/release_chs/ex3', 'FFXIV China - Retail - ex3 (Shadowbringers) - Win32', '994c6c3b', 3),
       (16, 'shanda/win32/release_chs/ex4', 'FFXIV China - Retail - ex4 (Endwalker) - Win32', '0728f998', 3),
       (17, 'ffxivneo/win32/release/ex5', 'FFXIV Global/JP - Retail - ex5 (Dawntrail) - Win32', '6cfeab11', 1),
       (18, 'actoz/win32/release_ko/ex5', 'FFXIV Korea - Retail - ex5 (Dawntrail) - Win32', '5050481e', 2),
       (19, 'shanda/win32/release_chs/ex5', 'FFXIV China - Retail - ex5 (Dawntrail) - Win32', '702fc90e', 3);

-- expansion
INSERT INTO expansion (id,
                       name_en,
                       name_ja,
                       name_de,
                       name_fr,
                       name_ko,
                       name_cn,
                       name_tw)
VALUES (0, 'A Realm Reborn', '新生エオルゼア', '', '', '', '', ''),
       (1, 'Heavensward', '蒼天のイシュガルド', '', '', '', '', ''),
       (2, 'Stormblood', '紅蓮の解放者', '', '', '', '', ''),
       (3, 'Shadowbringers', '漆黒の反逆者', '', '', '', '', ''),
       (4, 'Endwalker', '暁月の終焉', '', '', '', '', ''),
       (5, 'Dawntrail', '黄金の遺産', '', '', '', '', '');

-- expansion_repository_mapping
INSERT INTO expansion_repository_mapping (game_repository_id, expansion_id, expansion_repository_id)
VALUES (2, 0, 2),
       (2, 1, 3),
       (2, 2, 4),
       (2, 3, 5),
       (2, 4, 6),
       (7, 0, 7),
       (7, 1, 8),
       (7, 2, 9),
       (7, 3, 10),
       (7, 4, 11),
       (12, 0, 12),
       (12, 1, 13),
       (12, 2, 14),
       (12, 3, 15),
       (12, 4, 16),
       (2, 5, 17),
       (7, 5, 18),
       (12, 5, 19);