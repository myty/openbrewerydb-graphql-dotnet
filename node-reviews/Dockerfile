FROM node:14.15.1-alpine
ENV NODE_ENV=development

WORKDIR /app

COPY ["package.json", "package-lock.json*", "./"]

RUN npm install

COPY . .

RUN npm run build-ts

CMD [ "npm", "run", "watch" ]
