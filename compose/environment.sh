#!/usr/bin/env bash
export PORTAINER_VOL=$(pwd)/docker-data/portainer
export RABBITMQ_VOL=$(pwd)/docker-data/rabbitmq
export POSTGRES_VOL=$(pwd)/docker-data/postgres
export MINIO_VOL=$(pwd)/docker-data/minio
export NGINX_VOL=$(pwd)/docker-data/nginx
export CERTBOT_VOL=$(pwd)/docker-data/certbot
export REDIS_VOL=$(pwd)/docker-data/redis
export MONGO_VOL=$(pwd)/docker-data/mongo

# $(docker run --rm httpd:2.4-alpine htpasswd -nbB admin 'Uj8xasbE0iE1B66aTWAvzclIOrviUrRVhHnKvnRtN8Y5KyyXo5' | cut -d ":" -f 2)
# $2y$05$JvbTrH4ttme3Qre0eAibz.1n6PKU2Kcien77Myfd4Tn.EdMdNfRKq
export PORTAINER_PASSWORD='$2y$05$JvbTrH4ttme3Qre0eAibz.1n6PKU2Kcien77Myfd4Tn.EdMdNfRKq'

export RABBITMQ_USERNAME='admin'
export RABBITMQ_PASSWORD='9vxTUANFy7b4NnZay4SiGZuagwMmlEtSwL1UFoYi8BXTLzur6q'

export MINIO_USERNAME='admin'
export MINIO_PASSWORD='zFtfQH39yNDpRZq3zRU4pXI5C3xL7WKcy2GOR139AwJ9amjaIp'

export POSTGRES_USERNAME='oVglOEvDs1DO6chzz'
export POSTGRES_PASSWORD='bz4EAsdqYiUc19xJqRnStA83CLmj0j1OrgxDDVc1ApUKtqmLmL'

export REDIS_PASSWORD='INN735W4QJfF651rri1rL8bVXRzInpv48Cix5XR7kwsS0KFt0K'

# https://www.digitalocean.com/community/tutorials/how-to-set-up-password-authentication-with-nginx-on-ubuntu-14-04
# sudo sh -c "echo -n 'mongo_admin:' >> /etc/nginx/.htpasswd"
# sudo sh -c "openssl passwd -apr1 >> /etc/nginx/.htpasswd"
# enter this password --> S594S6Gp6ia8whaQd2IYnt0ETrRtrLLDEaazDD6hrs2ilQ4x2G
export MONGO_USERNAME='oVglOEvDs1DO6chzz'
export MONGO_PASSWORD='pv4OCqZVfo8m54dlu7WANP7bU36J22CTjfuKYTbAJz68n11p0L'