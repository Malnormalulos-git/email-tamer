FROM mysql:8.3.0

ENV MYSQL_USER emailtameruser
ENV MYSQL_PASSWORD emailtamerpassword
ENV MYSQL_DATABASE emailtamer
ENV MYSQL_RANDOM_ROOT_PASSWORD true

COPY init.sql /docker-entrypoint-initdb.d/