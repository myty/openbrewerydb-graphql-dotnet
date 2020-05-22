FROM node:alpine as node

WORKDIR /src
COPY . .

RUN yarn install && yarn build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=node /src/build /usr/share/nginx/html
