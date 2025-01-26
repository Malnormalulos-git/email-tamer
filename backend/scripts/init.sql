GRANT ALL PRIVILEGES ON *.* TO 'emailtameruser'@'%' WITH GRANT OPTION;
ALTER USER 'emailtameruser'@'%' IDENTIFIED BY 'emailtamerpassword';
FLUSH PRIVILEGES;