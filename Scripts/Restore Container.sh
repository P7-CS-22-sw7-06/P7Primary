docker create --name worker2 --security-opt seccomp:unconfined busybox /bin/sh -c 'i=0; while true; do echo $i; i=$(expr $i + 1); sleep 1; done'

docker start --checkpoint checkpoint worker2
