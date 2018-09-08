#ifndef SHANGHAI_EXEC_H
#define SHANGHAI_EXEC_H

#include <memory>
#include <initializer_list>
#include <string>
#include <thread>

namespace shanghai {

struct PipeDeleter {
	void operator()(int *fds);
};
using Pipe = std::unique_ptr<int[], PipeDeleter>;

class Process final {
public:
	Process(const std::string &path, std::initializer_list<std::string> argv);
	~Process();
	Process(const Process &) = delete;
	Process & operator=(const Process &) = delete;

	void Kill();
	void WaitForExit();
	void InputAndClose(const std::string &data);
	const std::string &GetOut();
	const std::string &GetErr();

private:
	Pipe CreatePipe();

	int m_pid;
	bool m_exit;
	Pipe m_in, m_out, m_err;
	std::thread m_outth, m_errth;
	std::string m_outbuf, m_errbuf;
};

}	// namespace shanghai

#endif	// SHANGHAI_EXEC_H
