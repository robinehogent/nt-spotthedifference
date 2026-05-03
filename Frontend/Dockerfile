FROM node:20-alpine AS build
WORKDIR /app

# allow VITE_API_URL to be set in .env of docker-compose.yml
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:alpine AS runtime

# copy the nginx template
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

COPY --from=build /app/dist /usr/share/nginx/html

