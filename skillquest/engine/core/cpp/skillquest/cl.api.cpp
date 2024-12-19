/**
 * @author omnisudo
 * @date 2024.08.08
 */
#include "skillquest/cl.api.hpp"

namespace sq {
    skillquest::ClientAPI*& client() {
        return skillquest::ClientAPI::instance();
    }
}// namespace sq