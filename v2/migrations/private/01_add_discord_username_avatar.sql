-- Add discord username and avatar fields to user table
ALTER TABLE user ADD COLUMN discord_username TEXT;
ALTER TABLE user ADD COLUMN discord_avatar TEXT;
