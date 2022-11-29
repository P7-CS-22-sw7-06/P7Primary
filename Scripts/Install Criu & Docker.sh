# -------------------- 64 bit --------------------

cd ~

# This one is annoying
sudo apt remove unattended-upgrades

sudo apt update -y && sudo apt upgrade -y

sudo add-apt-repository -y universe
sudo add-apt-repository -y multiverse

sudo apt install -y build-essential python-is-python3 python3-pip

sudo apt update -y && sudo apt upgrade -y

git clone https://github.com/checkpoint-restore/criu

cd criu

sudo apt install -y libprotobuf-dev libprotobuf-c-dev protobuf-c-compiler \
  protobuf-compiler python3-protobuf

pip install ipaddress

sudo apt install -y pkg-config libbsd-dev iproute2 libnftables-dev libcap-dev \
  libnet1-dev libnl-3-dev libnet-dev libaio-dev libgnutls28-dev python3-future

sudo apt install -y asciidoc xmlto

sudo apt update -y && sudo apt upgrade -y

make
sudo make install

# -------------------- Docker --------------------

sudo apt update -y && sudo apt upgrade -y
sudo apt-get install ca-certificates curl gnupg lsb-release

sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
  sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt update -y && sudo apt upgrade -y

DOCKER_VERSION=$(apt-cache madison docker-ce | awk '{ print $3 }' | grep '17')

sudo apt-get install docker-ce=${DOCKER_VERSION} \
  docker-ce-cli=${DOCKER_VERSION} containerd.io docker-compose-plugin

sudo apt-mark hold docker-ce
sudo apt-mark hold docker-ce-cli

sudo echo "{\"experimental\": true}" >> /etc/docker/daemon.json
sudo mv daemon.json /etc/docker/daemon.json
