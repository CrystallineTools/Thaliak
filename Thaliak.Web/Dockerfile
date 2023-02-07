FROM node:16-alpine AS build
RUN apk add git

ENV NODE_ENV production
WORKDIR /app

# so we can get the commit hash...
COPY .git .
COPY Thaliak.Web/ .

RUN yarn install --immutable && yarn build

FROM nginx:alpine AS web
COPY --from=build /app/build /var/www/html
COPY Thaliak.Web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 5000
CMD ["nginx", "-g", "daemon off;"]
