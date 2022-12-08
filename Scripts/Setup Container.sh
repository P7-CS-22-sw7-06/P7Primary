docker run -d --name worker busybox /bin/sh -c 'i=0; while true; do echo $i; i=$(expr $i + 1); sleep 1; done'

# containerid = sudo docker ps -aqf "name=worker"

# docker cp containerid:/script.py script.py
