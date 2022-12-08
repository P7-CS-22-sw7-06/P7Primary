sudo docker login ghcr.io 

mkdir .docker

json_data=$(cat <<EOF
{
	"auths": {
		"ghcr.io/p7-cs-22-sw7-06/p7primary": {
			"auth": ""
		}
	}
}
EOF
)

cd .docker
echo "$json_data" > config.json
cd
sudo docker run -d   --name watchtower --restart unless-stopped  -v $HOME/.docker/config.json:/config.json   -v /var/run/docker.sock:/var/run/docker.sock   containrrr/watchtower -i 120 --cleanup
sudo docker run -d --name workr ghcr.io/p7-cs-22-sw7-06/p7primary:master

sudo docker ps