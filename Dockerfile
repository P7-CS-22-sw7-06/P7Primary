# syntax=docker/dockerfile:1

FROM python:3.8-slim-buster
ARG TARGETOS
ARG TARGETARCH

#Set Relative Working Directory
WORKDIR /app

#Copy Requirements
COPY . .

#Install Packages
RUN pip3 install -r requirements.txt

#Copy all files from current direcory to image
COPY . .

CMD [ "python3","-u","./app.py"]

