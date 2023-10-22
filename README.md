# Running Instructions
1. `docker pull mysql`
2. `docker run --rm --name my-project-mysql -e MYSQL_ROOT_PASSWORD=pw123 -p 3406:3306 -d mysql`
   * Now you can connect to mysql on `127.0.0.1:3406`
3. Run the script in `db_setup.sql`
   * Creates schema `MyAppDatabase`
   * Creates table `MyData`
   * Creates procedure `GetOrCreateDataId`