FROM mysql:9.7

# Local testing defaults — override via docker-compose or environment variables.
ENV MYSQL_ROOT_PASSWORD=root
ENV MYSQL_DATABASE=blogger
ENV MYSQL_USER=blogger
ENV MYSQL_PASSWORD=blogger

COPY init/ /docker-entrypoint-initdb.d/
