#!/bin/sh
docker compose --profile run-tests up --build --exit-code-from tests
