sudo docker login ghcr.io -u Jacopopsen -p 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

mkdir .docker

json_data=$(cat <<EOF
{
	"auths": {
		"ghcr.io/p7-cs-22-sw7-06/p7primary": {
			"auth": "amFjb3BvcHNlbjpnaHBfMEI2SVFoMm9qN1B5aGV5dWkzS2E1SVk2UU9JZGQ5MEUzeVkx"
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