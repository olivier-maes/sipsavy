package application

import (
	"log/slog"
	"strconv"
	"time"
)

const (
	LogLevelEnvVar               = "LOG_LEVEL"
	ServerListenAddressEnvVar    = "SERVER_LISTEN_ADDRESS"
	DatabaseURLEnvVar            = "DATABASE_URL"
	IdleTimeoutEnvVar            = "IDLE_TIMEOUT"
	WriteTimeoutEnvVar           = "WRITE_TIMEOUT"
	ReadHeaderTimeoutEnvVar      = "READ_HEADER_TIMEOUT"
	SessionManagerLifetimeEnvVar = "SESSION_MANAGER_LIFETIME"
)

type loadEnvFn func(string) (string, bool)

type Config struct {
	logLevel               slog.Level
	serverListenAddress    string
	databaseURL            string
	idleTimeout            time.Duration
	readHeaderTimeout      time.Duration
	writeTimeout           time.Duration
	sessionManagerLifetime time.Duration
}

func NewConfig(loadFromEnv loadEnvFn) Config {
	config := Config{}

	config.logLevel = loadLogLevelFromEnvironment(loadFromEnv)
	config.serverListenAddress = loadStringFromEnv(loadFromEnv, ServerListenAddressEnvVar, "localhost:8080")
	config.databaseURL = loadStringFromEnv(loadFromEnv, DatabaseURLEnvVar, "")
	config.idleTimeout = loadSecondsDurationFromEnv(loadFromEnv, IdleTimeoutEnvVar, time.Minute)
	config.readHeaderTimeout = loadSecondsDurationFromEnv(loadFromEnv, ReadHeaderTimeoutEnvVar, 5*time.Second)
	config.writeTimeout = loadSecondsDurationFromEnv(loadFromEnv, WriteTimeoutEnvVar, 10*time.Second)
	config.sessionManagerLifetime = loadHoursDurationFromEnv(loadFromEnv, SessionManagerLifetimeEnvVar, 12*time.Hour)

	return config
}

func loadLogLevelFromEnvironment(loadFromEnv loadEnvFn) slog.Level {
	logLevelString, found := loadFromEnv(LogLevelEnvVar)
	if !found {
		logLevelString = "info"
	}

	var lvl slog.Level
	err := lvl.UnmarshalText([]byte(logLevelString))
	if err != nil {
		lvl = slog.LevelInfo
	}

	return lvl
}

func loadStringFromEnv(loadFromEnv loadEnvFn, key string, defaultValue string) string {
	value, found := loadFromEnv(key)
	if !found {
		return defaultValue
	}
	return value
}

func loadSecondsDurationFromEnv(loadFromEnv loadEnvFn, key string, defaultValue time.Duration) time.Duration {
	value, found := loadFromEnv(key)
	if !found {
		return defaultValue
	}
	intValue, err := strconv.Atoi(value)
	if err != nil {
		return defaultValue
	}

	return time.Duration(intValue * int(time.Second))
}

func loadHoursDurationFromEnv(loadFromEnv loadEnvFn, key string, defaultValue time.Duration) time.Duration {
	value, found := loadFromEnv(key)
	if !found {
		return defaultValue
	}
	intValue, err := strconv.Atoi(value)
	if err != nil {
		return defaultValue
	}

	return time.Duration(intValue * int(time.Hour))
}
