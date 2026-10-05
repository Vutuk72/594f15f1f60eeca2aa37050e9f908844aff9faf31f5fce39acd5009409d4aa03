# 1. fáze: sestavení Angularu v Node.js
FROM node:24-alpine AS build
WORKDIR /app

# Nejdřív jen package*.json a instalace – vrstva se cachuje, dokud se nezmění závislosti.
COPY package.json package-lock.json ./
RUN npm ci

COPY . ./
RUN npm run build

# 2. fáze: hotové statické soubory servíruje nginx (Node.js už není potřeba)
FROM nginx:alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
