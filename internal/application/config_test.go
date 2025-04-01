package application

import (
	"log/slog"
	"reflect"
	"testing"
	"time"
)

func TestNewConfig(t *testing.T) {
	tests := []struct {
		name           string
		loadEnvFn      func(string) (string, bool)
		expectedConfig Config
	}{
		{
			name: "Load env vars from simulated environment",
			loadEnvFn: func(key string) (string, bool) {
				switch key {
				case LogLevelEnvVar:
					return "debug", true
				case ServerListenAddressEnvVar:
					return "localhost:8888", true
				case DatabaseURLEnvVar:
					return "postgres://postgres:postgres@localhost:5432/postgres?sslmode=disable", true
				case IdleTimeoutEnvVar:
					return "50", true
				case ReadHeaderTimeoutEnvVar:
					return "10", true
				case WriteTimeoutEnvVar:
					return "20", true
				case SessionManagerLifetimeEnvVar:
					return "24", true
				default:
					return "", false
				}
			},
			expectedConfig: Config{
				logLevel:               slog.LevelDebug,
				serverListenAddress:    "localhost:8888",
				databaseURL:            "postgres://postgres:postgres@localhost:5432/postgres?sslmode=disable",
				idleTimeout:            50 * time.Second,
				readHeaderTimeout:      10 * time.Second,
				writeTimeout:           20 * time.Second,
				sessionManagerLifetime: 24 * time.Hour,
			},
		},
		{
			name: "Test config default values",
			loadEnvFn: func(_ string) (string, bool) {
				return "", false
			},
			expectedConfig: Config{
				logLevel:               slog.LevelInfo,
				serverListenAddress:    "localhost:8080",
				databaseURL:            "",
				idleTimeout:            60 * time.Second,
				readHeaderTimeout:      5 * time.Second,
				writeTimeout:           10 * time.Second,
				sessionManagerLifetime: 12 * time.Hour,
			},
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			config := NewConfig(test.loadEnvFn)

			if !reflect.DeepEqual(test.expectedConfig, config) {
				t.Errorf("\nExpected:\t%+v\nGot:\t\t%+v\n", test.expectedConfig, config)
			}
		})
	}
}
