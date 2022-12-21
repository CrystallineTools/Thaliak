FROM node:16-alpine AS build
RUN apk add git

ENV NODE_ENV production
WORKDIR /app
COPY . .
RUN yarn install --immutable && yarn build

FROM nginx:alpine AS web
COPY --from=build /app/build /var/www/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
